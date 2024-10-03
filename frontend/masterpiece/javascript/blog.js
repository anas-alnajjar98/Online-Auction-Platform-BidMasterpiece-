document.addEventListener("DOMContentLoaded", function() {
    const blogContainer = document.getElementById('blogContainer');
    const paginationContainer = document.getElementById('pagination');

    let currentPage = 1; // Default to page 1

    // Function to fetch blog data from API
    async function fetchBlogs(pageNumber) {
        event.preventDefault();
        debugger
        try {
            const response = await fetch(`https://localhost:7046/api/Home/GetAllBlogs?pageNumber=${pageNumber}&pageSize=6`);
            const data = await response.json();
            const blogs = data.blogs;
            const totalPages = data.totalPages;

           
            blogContainer.innerHTML = '';

           
            blogs.forEach(blog => {
                const blogCard = `
                    <div class="col-md-6 col-lg-4">
                        <div class="card blog-card">
                            <img class="card-img-top" src="assets/images/${blog.imageUrl}" alt="blog-image">
                            <div class="card-body p-3 p-md-4">
                                <a href="blog-detail.html?blogId=${blog.blogId}">${blog.title}</a>
                                <p>${blog.content}</p>
                                <div class="d-flex align-items-center justify-content-between">
                                    <div class="d-flex align-items-center gap-1">
                                        <img src="${blog.authorAvatar}" height="50" class="blog-avatar" alt="avatar">
                                        <h6>${blog.authorName}</h6>
                                    </div>
                                    <p class="mb-0"><span>${blog.viewCount}</span> views</p>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                blogContainer.innerHTML += blogCard;
            });

           
            updatePagination(totalPages, pageNumber);
        } catch (error) {
            console.error('Error fetching blog data:', error);
        }
    }

    
    function updatePagination(totalPages, currentPage) {
        paginationContainer.innerHTML = ''; 

        let paginationHtml = '';

        
        paginationHtml += `
            <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" aria-label="Previous" data-page="${currentPage - 1}">
                    <span aria-hidden="true">«</span>
                </a>
            </li>
        `;

        
        for (let i = 1; i <= totalPages; i++) {
            paginationHtml += `
                <li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>
            `;
        }

        
        paginationHtml += `
            <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link" href="#" aria-label="Next" data-page="${currentPage + 1}">
                    <span aria-hidden="true">»</span>
                </a>
            </li>
        `;

        paginationContainer.innerHTML = paginationHtml;

        // Add event listeners to pagination links
        paginationContainer.querySelectorAll('.page-link').forEach(link => {
            link.addEventListener('click', function(event) {
                event.preventDefault();
                const page = parseInt(event.target.getAttribute('data-page'));
                if (!isNaN(page) && page >= 1 && page <= totalPages) {
                    fetchBlogs(page); // Fetch blogs for the selected page
                }
            });
        });
    }

    // Initial fetch for the first page
    fetchBlogs(currentPage);
});
