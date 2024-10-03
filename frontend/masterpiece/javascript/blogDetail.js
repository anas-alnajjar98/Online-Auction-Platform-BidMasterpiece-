document.addEventListener("DOMContentLoaded", function() {
    

    // Function to get the query parameter value (blogId in this case)
    function getQueryParam(param) {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(param);
    }

    // Get the blogId from the URL
    const blogId = getQueryParam('blogId');

    // Function to fetch blog details using the blogId
    async function fetchBlogDetails(blogId) {
        debugger
        try {
            const response = await fetch(`https://localhost:7046/api/Home/GetBlogById/${blogId}`);
            const blog = await response.json();

            // Update the HTML with the blog data
            document.getElementById('blogTitle').innerText = blog.title;
            document.getElementById('blogContent').innerText = blog.content;
            document.getElementById('blogImage').src = `assets/images/${blog.imageUrl}`;
            updateSocialMediaLinks(blog.title);

        } catch (error) {
            console.error('Error fetching blog details:', error);
            document.getElementById('blogTitle').innerText = "Blog Not Found";
            document.getElementById('blogContent').innerText = "We couldn't find the blog you're looking for.";
        }
    }

    // Function to update social media share links
    function updateSocialMediaLinks(blogTitle) {
        debugger;
        const blogUrl = window.location.href;

        // URLs for sharing on social media platforms
        const facebookUrl = `https://www.facebook.com/sharer.php?u=${blogUrl}&t=${blogTitle}`;
        const twitterUrl = `https://twitter.com/intent/tweet?url=${blogUrl}&text=${blogTitle}`;
        const pinterestUrl = `https://pinterest.com/pin/create/button/?url=${blogUrl}&description=${blogTitle}`;
        const whatsappUrl = `https://api.whatsapp.com/send?text=${blogTitle} ${blogUrl}`;
        const instagramUrl = `https://www.instagram.com/?url=${blogUrl}`;

        document.getElementById('share-facebook').setAttribute('href', facebookUrl);
        document.getElementById('share-twitter').setAttribute('href', twitterUrl);
        document.getElementById('share-pinterest').setAttribute('href', pinterestUrl);
        document.getElementById('share-whatsapp').setAttribute('href', whatsappUrl);
        document.getElementById('share-instagram').setAttribute('href', instagramUrl);

    }

   
    if (blogId) {
        fetchBlogDetails(blogId);
    } else {
        document.getElementById('blogTitle').innerText = "Blog Not Found";
        document.getElementById('blogContent').innerText = "Please go back and select a blog.";
    }
});
